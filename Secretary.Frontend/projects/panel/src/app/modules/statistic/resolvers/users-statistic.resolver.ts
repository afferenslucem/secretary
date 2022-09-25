import { Injectable } from '@angular/core';
import { Resolve } from '@angular/router';
import { firstValueFrom, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { StatisticRow } from '../models/statistic-row';
import { UsersStatistic } from '../models/users-statistic';

@Injectable({
  providedIn: 'root'
})
export class UsersStatisticResolver implements Resolve<UsersStatistic> {
  public constructor(private httpClient: HttpClient) {
  }

  resolve(): Observable<UsersStatistic> {
    return this.httpClient.get<UsersStatistic>('/statistic/users')
  }
}
