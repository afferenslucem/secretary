import { ChangeDetectionStrategy, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { Chart, ChartData } from 'chart.js';
import { StatisticRow } from '../../models/statistic-row';

@Component({
  selector: 'app-pie-chart',
  templateUrl: './pie-chart.component.html',
  styleUrls: ['./pie-chart.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PieChartComponent implements OnInit {

  @Input()
  public header!: string;

  @Input()
  public borderColor?: string;

  @Input()
  public data!: StatisticRow[];

  public totalCount!: number;

  @ViewChild('chart', {static: true})
  public canvas!: ElementRef<HTMLCanvasElement>;
  private chart!: Chart;

  constructor() { }

  ngOnInit(): void {
    this.chart = new Chart(this.canvas.nativeElement, {
      type: 'pie',
      data: this.getDataConfig()
    });

    this.totalCount = this.data.map(item => item.count).reduce((acc, item) => acc + item, 0);
  }

  getDataConfig(): ChartData {
    return {
      labels: this.data.map(item => item.displayName),
      datasets: [{
        backgroundColor: this.data.map(item => item.color),
        borderColor: this.borderColor,
        borderWidth: 1,
        data: this.data.map(item => item.count),
      }]
    }
  }
}
