import { ChangeDetectorRef, Component, OnInit } from "@angular/core";
import {
  FormBuilder,
  FormGroup,
  Validators,
} from "@node_modules/@angular/forms";
import {
  SourceConfiguationDto,
  SourceConfiguationServiceProxy,
} from "@shared/service-proxies/service-proxies";

@Component({
  selector: "app-schedule-backup",
  templateUrl: "./schedule-backup.component.html",
  styleUrl: "./schedule-backup.component.css",
})
export class ScheduleBackupComponent implements OnInit {
  scheduleForm!: FormGroup;
  cronExpression: string = "";
  showDayOfWeek = false;
  showDayOfMonth: boolean;
  backupConfigs: any[] = []; // Backup configurations

  daysOfMonth: { label: string; value: number }[] = [];
  frequencies = [
    { label: "Daily", value: "daily" },
    { label: "Weekly", value: "weekly" },
    { label: "Monthly", value: "monthly" },
  ];

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

  constructor(
    private fb: FormBuilder,
    private sourceConfigService: SourceConfiguationServiceProxy,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadSourceConfigs();
    this.scheduleForm = this.fb.group({
      frequency: ["", Validators.required],
      dayOfWeek: [null],
      dayOfMonth: [null],
      time: ["", Validators.required],
    });
    this.scheduleForm.get("frequency")?.valueChanges.subscribe((value) => {
      this.showDayOfWeek = value === "weekly";
    });

    this.daysOfMonth = Array.from({ length: 31 }, (v, i) => ({
      label: `${i + 1}${this.getDaySuffix(i + 1)}`,
      value: i + 1,
    }));
  }

  loadSourceConfigs(): void {
    this.sourceConfigService.getAll(undefined, undefined, 1000, 0).subscribe({
      next: (result) => {
        if (result && result.items) {
          debugger;
          this.backupConfigs = result.items.map((config: any) => ({
            backupName: config.backupName,
            backUpStorageConfiguationId: config.backUpStorageConfiguationId,
          }));

          this.cdr.detectChanges();
        }
      },
      error: (err) => {
        console.error("Error fetching Source Configurations:", err);
      },
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
      const frequencyValue = this.scheduleForm.value.frequency?.value ?? "*";
      const dayOfWeekValue = this.scheduleForm.value.dayOfWeek?.value ?? "*";
      const time = this.scheduleForm.value.time;
      const dayOfMonthValue = this.scheduleForm.value.dayOfMonth?.value ?? "*";
      this.cronExpression = this.generateCron(
        frequencyValue,
        dayOfWeekValue,
        time,
        dayOfMonthValue
      );
      this.scheduleTask(this.cronExpression);

      const formData = {
        ...this.scheduleForm.value,
        cronExpression: this.cronExpression,
      };

      console.log("Final Payload:", formData);
    } else {
      this.scheduleForm.markAllAsTouched();
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
      case "daily":
        return `${minute} ${hour} * * *`; // Every day at the specified time
      case "weekly":
        return `${minute} ${hour} * * ${dayOfWeek !== null ? dayOfWeek : "*"}`; // Use dayOfWeek if available
      case "monthly":
        return `${minute} ${hour} ${
          dayOfMonth !== null ? dayOfMonth : "*"
        } * *`; // Use dayOfMonth if available
      default:
        return "";
    }
  }

  onFrequencyChange(event: any): void {
    debugger;
    const selectedValue = event.value?.value;

    this.showDayOfWeek = selectedValue === "weekly";
    this.showDayOfMonth = selectedValue === "monthly";

    if (selectedValue !== "weekly") {
      this.scheduleForm.get("dayOfWeek")?.setValue(null);
    }
    if (selectedValue !== "monthly") {
      this.scheduleForm.get("dayOfMonth")?.setValue(null);
    }
  }

  scheduleTask(cronExpression: string): void {
    console.log(`Task scheduled with cron: ${cronExpression}`);
  }
}
