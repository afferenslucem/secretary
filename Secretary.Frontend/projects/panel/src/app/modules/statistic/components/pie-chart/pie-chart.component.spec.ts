import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PieChartComponent } from './pie-chart.component';
import { StatisticRow } from '../../models/statistic-row';

describe('PieChartComponent', () => {
  let component: PieChartComponent;
  let fixture: ComponentFixture<PieChartComponent>;

  const statisticRows: StatisticRow[] = [
    {
      displayName: 'Отгул',
      count: 1,
      color: '#ADEEE3'
    },
    {
      displayName: 'Удаленка',
      count: 2,
      color: '#63B995'
    },
    {
      displayName: 'Отпуск',
      count: 3,
      color: '#86DEB7'
    }
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PieChartComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PieChartComponent);
    component = fixture.componentInstance;

    component.data = [];

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should set labels', () => {
    component.data = statisticRows;

    const result = component.getDataConfig();

    expect(result.labels).toEqual(['Отгул', 'Удаленка', 'Отпуск']);
  });

  it('should set data', () => {
    component.data = statisticRows;

    const result = component.getDataConfig();

    expect(result.datasets[0].data).toEqual([1, 2, 3]);
  });

  it('should set colors', () => {
    component.data = statisticRows;

    const result = component.getDataConfig();

    expect(result.datasets[0].backgroundColor).toEqual(['#ADEEE3', '#63B995', '#86DEB7']);
  });
});
