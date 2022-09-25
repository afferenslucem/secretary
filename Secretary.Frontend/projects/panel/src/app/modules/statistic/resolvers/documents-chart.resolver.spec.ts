import { TestBed } from '@angular/core/testing';

import { DocumentsChartResolver } from './documents-chart.resolver';
import { HttpClient } from '@angular/common/http';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of } from 'rxjs';

describe('DocumentsChartResolver', () => {
  let resolver: DocumentsChartResolver;
  let client: HttpClient;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule
      ],
    });
    resolver = TestBed.inject(DocumentsChartResolver);
    client = TestBed.inject(HttpClient);
  });

  it('should be created', () => {
    expect(resolver).toBeTruthy();
  });

  it('should send request', async () => {
    const spy = spyOn(client, "get").and.returnValue(of({ timeOffCount: 0, vacationCount: 0, distantCount: 0 }));

    await resolver.resolve();

    expect(spy).toHaveBeenCalledOnceWith('/statistic/documents')
  });

  it('should convert data', async () => {
    const spy = spyOn(client, "get").and.returnValue(of({ timeOffCount: 0, vacationCount: 1, distantCount: 2 }));

    const result = await resolver.resolve();

    expect(result[0]).toEqual({
      displayName: 'Отгул',
      count: 0,
      color: '#ADEEE3'
    })

    expect(result[1]).toEqual({
      displayName: 'Отпуск',
      count: 1,
      color: '#86DEB7'
    })

    expect(result[2]).toEqual({
      displayName: 'Удаленка',
      count: 2,
      color: '#63B995'
    })
  });
});
