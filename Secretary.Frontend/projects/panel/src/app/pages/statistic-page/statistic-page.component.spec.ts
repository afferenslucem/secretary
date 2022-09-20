import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StatisticPageComponent } from './statistic-page.component';
import { RouterTestingModule } from "@angular/router/testing";
import { ActivatedRoute } from "@angular/router";
import { Statistic } from "./models/i-statistic";

describe('StatisticPageComponent', () => {
  let component: StatisticPageComponent;
  let fixture: ComponentFixture<StatisticPageComponent>;

  beforeEach(async () => {
    const statistic: Statistic = {
      documentStatistic: { timeOffCount: 0, vacationCount: 0, distantCount: 0 },
      userStatistic: { totalUsers: 0, userWithDocuments: 0 },
    }

    await TestBed.configureTestingModule({
      imports: [ StatisticPageComponent ],
      providers: [
        RouterTestingModule,
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              data: {
                statistic
              }
            }
          }
        }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StatisticPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
