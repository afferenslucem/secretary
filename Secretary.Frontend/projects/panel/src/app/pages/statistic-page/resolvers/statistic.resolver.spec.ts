import { TestBed } from '@angular/core/testing';
import { StatisticResolver } from './statistic.resolver';
import { HttpClientTestingModule } from "@angular/common/http/testing";
import { HttpClient } from "@angular/common/http";

describe('StatisticResolver', () => {
  let resolver: StatisticResolver;
  let client: HttpClient;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule
      ],
    });
    resolver = TestBed.inject(StatisticResolver);
    client = TestBed.inject(HttpClient);
  });

  it('should be created', () => {
    expect(resolver).toBeTruthy();
  });

  it('should send request', async () => {
    const spy = spyOn(client, "get");

    await resolver.resolve({} as any, {} as any);

    expect(spy).toHaveBeenCalledOnceWith('/statistic')
  });
});
