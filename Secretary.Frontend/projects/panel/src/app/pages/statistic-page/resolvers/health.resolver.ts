import { Injectable } from '@angular/core';
import {
  Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, of } from 'rxjs';
import { Statistic } from "../models/statistic";
import { HttpClient } from "@angular/common/http";
import { Health } from "../models/health";

@Injectable({
  providedIn: 'root'
})
export class HealthResolver implements Resolve<Health> {
  public constructor(private httpClient: HttpClient) {
  }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<Health> {
    return this.httpClient.get<Health>('/health');
  }
}
