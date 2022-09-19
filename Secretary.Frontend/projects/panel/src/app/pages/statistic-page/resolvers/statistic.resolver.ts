import { Injectable } from '@angular/core';
import {
  Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, of } from 'rxjs';
import { Statistic } from "../models/i-statistic";
import { HttpClient } from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class StatisticResolver implements Resolve<Statistic> {
  public constructor(private httpClient: HttpClient) {
  }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<Statistic> {
    return this.httpClient.get<Statistic>('/statistic');
  }
}
