import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import { RouterModule, RouterOutlet, Routes } from "@angular/router";
import { StatisticPageComponent } from "./pages/statistic-page/statistic-page.component";
import { HTTP_INTERCEPTORS, HttpClientModule } from "@angular/common/http";
import { UrlInterceptor } from "./interceptors/url.interceptor";
import { HealthResolver } from "./pages/statistic-page/resolvers/health.resolver";
import { DocumentsChartResolver } from './modules/statistic/resolvers/documents-chart.resolver';
import { UsersStatisticResolver } from './modules/statistic/resolvers/users-statistic.resolver';

const routes: Routes = [
  {
    path: '',
    component: StatisticPageComponent,
    resolve: {
      documentsChartData: DocumentsChartResolver,
      usersStatistic: UsersStatisticResolver,
      health: HealthResolver,
    }
  }
]

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    RouterOutlet,
    RouterModule.forRoot(routes),
    HttpClientModule,
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: UrlInterceptor,
      multi: true,
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
