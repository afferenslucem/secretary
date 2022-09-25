import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
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
