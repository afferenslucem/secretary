import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StatisticPageComponent } from './statistic-page.component';
import { RouterTestingModule } from "@angular/router/testing";
import { ActivatedRoute } from "@angular/router";
import { Health } from "./models/health";

describe('StatisticPageComponent', () => {
  let component: StatisticPageComponent;
  let fixture: ComponentFixture<StatisticPageComponent>;

  beforeEach(async () => {
    const health: Health = {
      botHealthData: { version: '0', deployTime: '', pingTime: '', receivedMessages: 0},
      refresherHealthData: { version: '0', deployTime: '', pingTime: '', nextRefreshDate: '' },
      reminderHealthData: { version: '0', deployTime: '', pingTime: '', nextNotifyDate: '' },
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
                health,
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
