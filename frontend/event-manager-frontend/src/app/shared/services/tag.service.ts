import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, shareReplay } from 'rxjs';
import { Tag } from '../models/tag.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class TagService {
  private readonly url = `${environment.apiUrl}/tags`;
  private cache$?: Observable<Tag[]>;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Tag[]> {
    if (!this.cache$) {
      this.cache$ = this.http.get<Tag[]>(this.url).pipe(
        shareReplay(1)
      );
    }
    return this.cache$;
  }
}
