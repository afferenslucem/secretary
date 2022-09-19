import { TestBed } from '@angular/core/testing';

import { StatisticResolver } from './statistic.resolver';

describe('StatisticResolver', () => {
  let resolver: StatisticResolver;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    resolver = TestBed.inject(StatisticResolver);
  });

  it('should be created', () => {
    expect(resolver).toBeTruthy();
  });
});
