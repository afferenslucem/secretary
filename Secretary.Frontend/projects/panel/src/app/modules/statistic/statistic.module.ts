import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PieChartComponent } from './components/pie-chart/pie-chart.component';



@NgModule({
  declarations: [
    PieChartComponent,
  ],
  imports: [
    CommonModule,
  ],
  exports: [
    PieChartComponent,
  ]
})
export class StatisticModule { }
