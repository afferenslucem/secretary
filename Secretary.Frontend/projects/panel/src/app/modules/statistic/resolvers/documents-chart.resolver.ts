import { Injectable } from '@angular/core';
import { Resolve } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { StatisticRow } from '../models/statistic-row';

@Injectable({
  providedIn: 'root'
})
export class DocumentsChartResolver implements Resolve<StatisticRow[]> {
  public constructor(private httpClient: HttpClient) {
  }

  async resolve(): Promise<StatisticRow[]> {
    const data = await firstValueFrom(this.httpClient.get<{[key: string]: number}>('/statistic/documents'));

    const result: Array<StatisticRow> = [];

    result.push({
      displayName: 'Отгул',
      count: data['timeOffCount'],
      color: '#ADEEE3'
    });

    result.push({
      displayName: 'Отпуск',
      count: data['vacationCount'],
      color: '#86DEB7'
    });

    result.push({
      displayName: 'Удаленка',
      count: data['distantCount'],
      color: '#63B995'
    });

    return result;
  }
}
