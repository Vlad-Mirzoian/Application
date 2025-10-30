import { Component } from '@angular/core';
import {
  BehaviorSubject,
  Subject,
  switchMap,
  startWith,
  scan,
  catchError,
  of,
  map,
  merge,
  Observable,
} from 'rxjs';
import { marked } from 'marked';
import { AiService } from '../shared/services/ai.service';
import { CommonModule } from '@angular/common';
import { MarkdownModule } from 'ngx-markdown';
import { Store } from '@ngrx/store';
import { AppState } from './store';
import { selectIsSidebarOpen } from './store/sidebar.selectors';
import { closeSidebar } from './store/sidebar.actions';
import { FormsModule } from '@angular/forms';

interface Message {
  role: 'user' | 'assistant';
  content: string;
  contentHtml?: string;
}

@Component({
  selector: 'app-ai-sidebar',
  standalone: true,
  imports: [CommonModule, FormsModule, MarkdownModule],
  templateUrl: './ai-sidebar.component.html',
  styleUrls: ['./ai-sidebar.component.scss'],
})
export class AiSidebarComponent {
  isOpen$!: Observable<boolean>;

  private inputSubject = new BehaviorSubject<string>('');
  input$ = this.inputSubject.asObservable();

  private sendTrigger$ = new Subject<string>();

  private response$ = this.sendTrigger$.pipe(
    switchMap((question) =>
      this.aiService.ask(question).pipe(
        catchError(() => of({ response: 'Sorry, something went wrong.' })),
        map((response) => ({
          role: 'assistant' as const,
          content: response.response,
        }))
      )
    )
  );

  private userMessages$ = this.sendTrigger$.pipe(
    map((question) => ({
      role: 'user' as const,
      content: question,
    }))
  );

  messages$ = merge(this.userMessages$, this.response$).pipe(
    scan((acc: Message[], msg: Message) => {
      if (msg.role === 'assistant') {
        msg.contentHtml = String(marked.parse(msg.content, { async: false }));
      }
      return [...acc, msg];
    }, []),
    startWith([] as Message[])
  );

  isLoading$ = this.sendTrigger$.pipe(
    switchMap((question) =>
      this.aiService.ask(question).pipe(
        map(() => false),
        startWith(true),
        catchError(() => of(false))
      )
    ),
    startWith(false)
  );

  constructor(private aiService: AiService, private store: Store<AppState>) {
    this.isOpen$ = this.store.select(selectIsSidebarOpen);
  }

  close() {
    this.store.dispatch(closeSidebar());
  }

  send(event?: Event) {
    if (event) event.preventDefault();
    const question = this.inputSubject.value.trim();
    if (!question) return;
    this.sendTrigger$.next(question);
    this.inputSubject.next('');
  }

  updateInputEvent(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    this.inputSubject.next(value);
  }

  copy(content: string) {
    navigator.clipboard.writeText(content);
  }
}
