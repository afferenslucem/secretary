import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from "@angular/router";
import { Chart, registerables } from "chart.js";
import { Health } from "./models/health";
import { StatisticModule } from '../../modules/statistic/statistic.module';
import { StatisticRow } from '../../modules/statistic/models/statistic-row';
import { UsersStatistic } from '../../modules/statistic/models/users-statistic';

Chart.register(...registerables);

@Component({
  selector: 'app-statistic-page',
  standalone: true,
  imports: [CommonModule, StatisticModule],
  templateUrl: './statistic-page.component.html',
  styleUrls: ['./statistic-page.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class StatisticPageComponent {
  public documentsChartData: StatisticRow[];
  public usersStatistic: UsersStatistic;
  public health: Health;

  constructor(private activatedRoute: ActivatedRoute) {
    this.documentsChartData = activatedRoute.snapshot.data['documentsChartData'];
    this.usersStatistic = activatedRoute.snapshot.data['usersStatistic'];
    this.health = activatedRoute.snapshot.data['health'];
  }
}
