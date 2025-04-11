import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AppConsts } from '../AppConsts';
import { Observable } from 'rxjs';
import { HttpResponse } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class LocalBackupService {
  constructor(private httpClient: HttpClient) { }
  takeBackupLocal$(sourceConfigurationId?: string): Observable<HttpResponse<Blob>> {
    let params = new HttpParams();
    if (sourceConfigurationId) {
      params = params.set('sourceConfigurationId', sourceConfigurationId);
    }
    const url = AppConsts.remoteServiceBaseUrl + `/api/backup/download-local`;

    return this.httpClient.get(url, {
      params,
      responseType: 'blob',
      observe: 'response',
      headers: new HttpHeaders({
        'Accept': 'application/octet-stream',
      }),
    });
  }
}
