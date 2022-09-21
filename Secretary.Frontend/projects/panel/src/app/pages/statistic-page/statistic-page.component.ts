import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, ActivatedRouteSnapshot } from "@angular/router";
import { Statistic } from "./models/statistic";
import { Chart, registerables  } from "chart.js";
import { Health } from "./models/health";

Chart.register(...registerables);

@Component({
  selector: 'app-statistic-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './statistic-page.component.html',
  styleUrls: ['./statistic-page.component.scss']
})
export class StatisticPageComponent implements OnInit {
  public statistic: Statistic;
  public health: Health;

  @ViewChild('documentsChart', {static: true})
  public documentsChartCanvas!: ElementRef<HTMLCanvasElement>;
  private documentsChart!: Chart;

  @ViewChild('userChart', {static: true})
  public userChartCanvas!: ElementRef<HTMLCanvasElement>;

  constructor(private activatedRoute: ActivatedRoute) {
    this.statistic = activatedRoute.snapshot.data['statistic'];
    this.health = activatedRoute.snapshot.data['health'];
  }

  public ngOnInit(): void {
    this.createDocumentsChart();
    this.createUserChart();
  }

  private createDocumentsChart(): void {
    this.documentsChart = new Chart(this.documentsChartCanvas.nativeElement, {
      type: 'pie',
      data: {
        labels: ['Отгул', 'Отпуск', 'Удаленка'],
        datasets: [{
          backgroundColor: ['#ADEEE3', '#86DEB7', '#63B995'],
          borderColor: '#08605F',
          borderWidth: 1,
          data: [
            this.statistic.documentStatistic.timeOffCount,
            this.statistic.documentStatistic.vacationCount,
            this.statistic.documentStatistic.distantCount,
          ],
        }]
      }
    });
  }

  private createUserChart(): void {
    this.documentsChart = new Chart(this.userChartCanvas.nativeElement, {
      type: 'pie',
      data: {
        labels: ['С документами', 'Без документов>'],
        datasets: [{
          backgroundColor: ['#6290C8', '#376996'],
          borderColor: '#1D3461',
          borderWidth: 1,
          data: [
            this.statistic.userStatistic.userWithDocuments,
            this.statistic.userStatistic.totalUsers - this.statistic.userStatistic.userWithDocuments,
          ],
        }]
      }
    });
  }
}
