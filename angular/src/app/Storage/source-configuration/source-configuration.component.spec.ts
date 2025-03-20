import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SourceConfigurationComponent } from './source-configuration.component';

describe('SourceConfigurationComponent', () => {
  let component: SourceConfigurationComponent;
  let fixture: ComponentFixture<SourceConfigurationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SourceConfigurationComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SourceConfigurationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
