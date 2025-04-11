import { ChangeDetectorRef, Component, OnInit } from "@angular/core";
import {
  FormBuilder,
  FormGroup,
  Validators,
} from "@node_modules/@angular/forms";
import { MessageService } from "primeng/api";
import {
  BackUpFrequencyDto,
  BackUpFrequencyServiceProxy,
  BackUpScheduleCreateDto,
  BackUpScheduleServiceProxy,
  SourceConfiguationDto,
  SourceConfiguationServiceProxy,
} from "@shared/service-proxies/service-proxies";

@Component({
  selector: "app-schedule-backup",
  templateUrl: "./schedule-backup.component.html",
  styleUrl: "./schedule-backup.component.css",
})
export class ScheduleBackupComponent implements OnInit {
deleteScheduledBackup(_t71: any) {
throw new Error('Method not implemented.');
}
editScheduledBackup(_t71: any) {
throw new Error('Method not implemented.');
}
  scheduleForm!: FormGroup;
  cronExpression: string = "";
  showDayOfWeek = false;
  showDayOfMonth: boolean;
  backupConfigs: any[] = [];
  daysOfMonth: { label: string; value: number }[] = [];
  frequencies = [];
  daysOfWeek = [
    { label: "Sunday", value: 0 },
    { label: "Monday", value: 1 },
    { label: "Tuesday", value: 2 },
    { label: "Wednesday", value: 3 },
    { label: "Thursday", value: 4 },
    { label: "Friday", value: 5 },
    { label: "Saturday", value: 6 },
  ];
  sourceConfigs: SourceConfiguationDto[];
  isSaving: any;
  constructor(
    private fb: FormBuilder,
    private sourceConfigService: SourceConfiguationServiceProxy,
    private cdr: ChangeDetectorRef,
    private BackUPTypeService: BackUpFrequencyServiceProxy,
    private BackUpScheduleService: BackUpScheduleServiceProxy,
    private messageService: MessageService
  ) {}

  ngOnInit(): void {
    this.loadSourceConfigs();
    this.loadfrequencies();
    this.scheduleForm = this.fb.group({
      frequency: ["", Validators.required],
      dayOfWeek: [null],
      dayOfMonth: [null],
      time: ["", Validators.required],
      sourceConfiguationId: ["", Validators.required],
      backupDate: [null],
    });
    this.scheduleForm.get("frequency")?.valueChanges.subscribe((value) => {
      this.showDayOfWeek = value === "weekly";
    });

    this.daysOfMonth = Array.from({ length: 31 }, (v, i) => ({
      label: `${i + 1}${this.getDaySuffix(i + 1)}`,
      value: i + 1,
    }));
  }
  loadfrequencies(): void {
    this.BackUPTypeService.getAll().subscribe({
      next: (result) => {
        if (result && result.items) {
          this.frequencies = result.items.map((item: BackUpFrequencyDto) => ({
            label: item.name,
            value: item.id,
          }));
        }
      },
      error: (err) => {},
    });
  }
  loadSourceConfigs(): void {
    this.sourceConfigService.getAll(undefined, undefined, 1000, 0).subscribe({
      next: (result) => {
        if (result && result.items) {
          this.backupConfigs = result.items.map((config: any) => {
            const backUPType = config.backUPType?.name || "";
            const dbType = config.dbType?.name || "";
            const IP = config.serverIP || "";
            return {
              backUPType,
              dbType,
              backUpStorageConfiguationId: config.id,
              label: dbType ? `${backUPType} - ${dbType}` : `${backUPType} - ${IP}`
            };
          });
          this.cdr.detectChanges();
        }
      },
      error: (err) => {},
    });
  }
  getDaySuffix(day: number): string {
    if (day >= 11 && day <= 13) {
      return "th";
    }
    switch (day % 10) {
      case 1:
        return "st";
      case 2:
        return "nd";
      case 3:
        return "rd";
      default:
        return "th";
    }
  }

  onSubmit(): void {
    if (this.scheduleForm.valid) {
      this.isSaving = true;
      const frequencyValue = this.scheduleForm.value.frequency?.value ?? "*";
      const dayOfWeekValue = this.scheduleForm.value.dayOfWeek?.value ?? "*";
      const time = this.scheduleForm.value.time;
      const dayOfMonthValue = this.scheduleForm.value.dayOfMonth?.value ?? "*";
      this.cronExpression = this.generateCron(
        this.scheduleForm.value.frequency?.label,
        dayOfWeekValue,
        time,
        dayOfMonthValue
      );
      const formValue = this.scheduleForm.value;
      const formattedTime = formValue.time
        ? `${formValue.time}:00` // Append seconds to match "HH:mm:ss"
        : null;

      const payload = BackUpScheduleCreateDto.fromJS({
        sourceConfiguationId: formValue.sourceConfiguationId,
        backupDate: null,
        backupTime: formattedTime,
        backUpFrequencyId: formValue.frequency?.value,
        cronExpression: this.cronExpression,
      });
      payload.backupDate = null;
      this.BackUpScheduleService.create(payload).subscribe({
        next: (response) => {
          this.messageService.add({
            severity: "success",
            summary: "success",
            detail: "Scheduled successfully!",
            life: 2000,
          });
          this.loadSourceConfigs();
          this.loadfrequencies();
          this.scheduleForm.reset();
          this.isSaving = false;
          this.cronExpression = "";
          this.showDayOfWeek = false; 
        },
        error: (err) => {
          this.messageService.add({
            severity: "error",
            summary: "Error",
            detail: "Something went wrong!",
            life: 2000,
          });
          this.isSaving = false;
        },
      });
    } else {
      this.scheduleForm.markAllAsTouched();
      this.messageService.add({
        severity: "error",
        summary: "Error",
        detail: "All fields are required!",
        life: 2000,
      });
    }
  }

  generateCron(
    frequency: string,
    dayOfWeek: number | string,
    time: string,
    dayOfMonth: number | string
  ): string {
    const [hour, minute] = time.split(":");

    switch (frequency) {
      case "Hourly":
        return `${minute} * * * *`; 
      case "Daily":
        return `${minute} ${hour} * * *`;
      case "Weekly":
        return `${minute} ${hour} * * ${dayOfWeek !== null ? dayOfWeek : "*"}`; 
      case "Monthly":
        return `${minute} ${hour} ${
          dayOfMonth !== null ? dayOfMonth : "*"
        } * *`;
      default:
        return "";
    }
  }

  onFrequencyChange(event: any): void {
    const selectedValue = event.value?.label;
    this.showDayOfWeek = selectedValue === "Weekly";
    this.showDayOfMonth = selectedValue === "Monthly";
    if (selectedValue !== "weekly") {
      this.scheduleForm.get("dayOfWeek")?.setValue(null);
    }
    if (selectedValue !== "monthly") {
      this.scheduleForm.get("dayOfMonth")?.setValue(null);
    }
  }


  
  
}
