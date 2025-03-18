import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BackupLogsComponent } from './backup-logs.component';

describe('BackupLogsComponent', () => {
  let component: BackupLogsComponent;
  let fixture: ComponentFixture<BackupLogsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BackupLogsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BackupLogsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
