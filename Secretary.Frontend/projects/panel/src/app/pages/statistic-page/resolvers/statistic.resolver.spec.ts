import { TestBed } from '@angular/core/testing';
import { StatisticResolver } from './statistic.resolver';
import { HttpClientTestingModule } from "@angular/common/http/testing";

describe('StatisticResolver', () => {
  let resolver: StatisticResolver;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule
      ]
    });
    resolver = TestBed.inject(StatisticResolver);
  });

  it('should be created', () => {
    expect(resolver).toBeTruthy();
  });
});
