import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AppConsts } from '../AppConsts';

@Injectable({
  providedIn: 'root'
})
export class BackupService {
  constructor(private httpClient: HttpClient) {}

  downloadBackup(sourceConfigurationId: string, backUpFileName: string) {
    debugger;
    let params = new HttpParams()
      .set('sourceConfigurationId', sourceConfigurationId)
      .set('backUpFileName', backUpFileName);

    const url =AppConsts.remoteServiceBaseUrl + `/api/backup/download`;

    this.httpClient.get(url, {
      params: params,
      responseType: 'blob',
      observe: 'response',
      headers: new HttpHeaders({
        'Accept': 'application/octet-stream'
      })
    }).subscribe({
      next: response => {
        const blob = response.body!;
        const contentDisposition = response.headers.get('Content-Disposition');
        const filename = this.getFilenameFromDisposition(contentDisposition, backUpFileName);

        const link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        link.download = filename;
        link.target = '_blank';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);

        window.URL.revokeObjectURL(link.href);
      },
      error: err => {
        console.error('Download failed:', err);
      }
    });
  }

  private getFilenameFromDisposition(contentDisposition: string | null, defaultName: string): string {
    if (!contentDisposition) return defaultName;

    const match = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/.exec(contentDisposition);
    return match && match[1]
      ? decodeURIComponent(match[1].replace(/['"]/g, ''))
      : defaultName;
  }
}
