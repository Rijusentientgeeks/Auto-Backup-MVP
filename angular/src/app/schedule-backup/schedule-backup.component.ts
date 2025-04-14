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
  BackUpScheduleDto,
  BackUpScheduleDtoPagedResultDto,
  BackUpScheduleServiceProxy,
  SourceConfiguationDto,
  SourceConfiguationServiceProxy,
} from "@shared/service-proxies/service-proxies";
import Swal from "sweetalert2";
interface ScheduledBackup {
  id: string;
  config: string;
  schedule: string;
  status: string;
}
@Component({
  selector: "app-schedule-backup",
  templateUrl: "./schedule-backup.component.html",
  styleUrl: "./schedule-backup.component.css",
})
export class ScheduleBackupComponent implements OnInit {
  deleteScheduledBackup(_t71: any) {
    throw new Error("Method not implemented.");
  }
  editScheduledBackup(_t71: any) {
    throw new Error("Method not implemented.");
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
  scheduledBackups: ScheduledBackup[] = [];
  totalRecords: number = 0;
  loading: boolean = false;
  filterText: string = "";
  sorting: string = "";
  currentPage: number = 0;
  rowsPerPage: number = 10;
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
    this.loadBackups(this.currentPage);
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
          this.backupConfigs = result.items.filter(item => item.isUserLocalSystem === false);
        }
      },
      error: (err) => {},
    });
  }
  loadBackups(page: number = 0) {
    const skipCount = page * this.rowsPerPage;
    this.BackUpScheduleService.getAll(
      this.filterText || undefined,
      this.sorting || undefined,
      this.rowsPerPage,
      skipCount
    ).subscribe({
      next: (result: BackUpScheduleDtoPagedResultDto) => {
        this.scheduledBackups = this.mapToScheduledBackup(result?.items || []);
        this.totalRecords = result?.totalCount || 0;
        this.cdr.detectChanges(); // Force table to re-render

      },
      error: (error) => {
    
      },
    });
  }
  mapToScheduledBackup(items: BackUpScheduleDto[]): ScheduledBackup[] {
    return items.map((item) => ({
      id: item.id,
      config: item.sourceConfiguation?.backupName || "Unknown",
      schedule: this.getScheduleText(item),
      status: item.isDeleted ? "Inactive" : "Active",
    }));
  }
  getScheduleText(item: BackUpScheduleDto): string {
    const frequency = item.backUpFrequency?.name || "Unknown";
    const time = item.backupTime || "N/A";
    switch (frequency.toLowerCase()) {
      case "hourly":
        return (
          `Hourly` + (item.cronExpression ? ` - ${item.cronExpression}` : "")
        );
      case "daily":
        return `Daily - ` + item.cronExpression;
      case "weekly":
        return `Weekly - ` + item.cronExpression;
      case "monthly":
        return `Monthly - ` + item.cronExpression;
      case "yearly":
        return `Yearly - ` + item.cronExpression;
      default:
        return item.cronExpression || `${frequency} at ${time}`;
    }
  }
  deleteBackup(backup: ScheduledBackup) {
    Swal.fire({
      title: "Are you sure?",
      text: "This will permanently delete the configuration!",
      icon: "warning",
      showCancelButton: true,
      confirmButtonColor: "#d33",
      cancelButtonColor: "#3085d6",
      confirmButtonText: "Yes, delete it!",
    }).then((result) => {
      if (result.isConfirmed) {
        this.BackUpScheduleService.removeSchedule(backup.id).subscribe({
          next: () => {
            Swal.fire("Deleted!", "Deleted Successfully", "success");
            this.loadBackups(this.currentPage);
          },
          error: (error) => {
            Swal.fire("Error!", "Something Wrong!", "error");
          },
        });
      }
    });
  }
  applyFilter(event: Event) {
    this.filterText = (event.target as HTMLInputElement).value;
    this.currentPage = 0;
    this.loadBackups(this.currentPage);
  }

  onPageChange(event: any) {
    this.currentPage = event.page;
    this.loadBackups(this.currentPage);
  }

  onSort(event: any) {
    const sortField = event.field;
    const sortOrder = event.order === 1 ? "asc" : "desc";
    let apiField = sortField;
    if (sortField === "config") {
      apiField = "sourceConfiguationId";
    } else if (sortField === "schedule") {
      apiField = "backupTime";
    } else if (sortField === "status") {
      apiField = "isDeleted";
    }
    this.sorting = `${apiField} ${sortOrder}`;
    this.loadBackups(this.currentPage);
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
      const formattedTime = formValue.time ? `${formValue.time}:00` : null;

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
          this.loadBackups();
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
