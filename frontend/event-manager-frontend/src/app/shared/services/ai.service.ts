import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface AiRequestDto {
  question: string;
}

export interface AiResponseDto {
  response: string;
}

@Injectable({ providedIn: 'root' })
export class AiService {
  private url = `${environment.apiUrl}/ai/assist`;

  constructor(private http: HttpClient) {}

  ask(question: string): Observable<AiResponseDto> {
    return this.http.post<AiResponseDto>(this.url, {
      question,
    } as AiRequestDto);
  }
}
