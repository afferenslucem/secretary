import { TestBed } from '@angular/core/testing';

import { DocumentsChartResolver } from './documents-chart.resolver';
import { HttpClient } from '@angular/common/http';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of } from 'rxjs';
import { UsersStatisticResolver } from './users-statistic.resolver';

describe('UsersStatisticResolver', () => {
  let resolver: UsersStatisticResolver;
  let client: HttpClient;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule
      ],
    });
    resolver = TestBed.inject(UsersStatisticResolver);
    client = TestBed.inject(HttpClient);
  });

  it('should be created', () => {
    expect(resolver).toBeTruthy();
  });

  it('should send request', async () => {
    const spy = spyOn(client, "get");

    await resolver.resolve();

    expect(spy).toHaveBeenCalledOnceWith('/statistic/users')
  });
});
