import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from "@angular/common/http/testing";
import { HttpClient } from "@angular/common/http";
import { HealthResolver } from "./health.resolver";

describe('HealthResolver', () => {
  let resolver: HealthResolver;
  let client: HttpClient;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule
      ],
    });
    resolver = TestBed.inject(HealthResolver);
    client = TestBed.inject(HttpClient);
  });

  it('should be created', () => {
    expect(resolver).toBeTruthy();
  });

  it('should send request', async () => {
    const spy = spyOn(client, "get");

    await resolver.resolve({} as any, {} as any);

    expect(spy).toHaveBeenCalledOnceWith('/health')
  });
});
