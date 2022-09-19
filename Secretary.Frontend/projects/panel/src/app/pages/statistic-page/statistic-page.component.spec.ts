import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StatisticPageComponent } from './statistic-page.component';

describe('StatisticPageComponent', () => {
  let component: StatisticPageComponent;
  let fixture: ComponentFixture<StatisticPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ StatisticPageComponent ]
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
