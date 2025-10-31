import {
  Component,
  ElementRef,
  HostListener,
  OnInit,
  ViewChild,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import {
  Observable,
  BehaviorSubject,
  of,
  Subject,
  concat,
  timer,
  merge,
} from 'rxjs';
import {
  switchMap,
  catchError,
  tap,
  map,
  startWith,
  shareReplay,
} from 'rxjs/operators';
import { EventService, EventDto, CalendarEventDto } from '../event.service';
import { AuthService } from '../../auth/auth.service';
import { Tag } from '../../shared/models/tag.model';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { TagService } from '../../shared/services/tag.service';

interface EventAction {
  type: 'join' | 'leave';
  id: string;
}

interface EventActionMessage {
  success: boolean;
  message: string;
}

@Component({
  selector: 'app-events-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatIconModule,
    ReactiveFormsModule,
  ],
  templateUrl: './events-list.component.html',
  styleUrls: ['./events-list.component.scss'],
})
export class EventsListComponent implements OnInit {
  Number = Number;
  events$!: Observable<EventDto[]>;
  userEvents$!: Observable<CalendarEventDto[]>;
  submitResult$!: Observable<EventActionMessage | null>;
  temporaryMessage$: Observable<string | null> = of(null);
  isAuthenticated: boolean;

  private refreshUserEvents$ = new BehaviorSubject<void>(undefined);
  private refreshEvents$ = new BehaviorSubject<void>(undefined);
  private actionTrigger$ = new Subject<EventAction>();

  tags$!: Observable<Tag[]>;
  selectedTags = new FormControl<string[]>([]);
  filteredEvents$!: Observable<EventDto[]>;
  dropdownOpen = false;

  constructor(
    private eventService: EventService,
    private authService: AuthService,
    private tagService: TagService,
    private router: Router
  ) {
    this.isAuthenticated = this.authService.isAuthenticated();
    const nav = this.router.getCurrentNavigation();
    const stateMessage = nav?.extras?.state?.['message'] || null;
    this.temporaryMessage$ = stateMessage
      ? concat(of(stateMessage), timer(3000).pipe(map(() => null)))
      : of(null);
  }

  ngOnInit() {
    this.tags$ = this.tagService.getAll();

    this.events$ = merge(
      this.selectedTags.valueChanges.pipe(
        startWith(this.selectedTags.value || [])
      ),
      this.refreshEvents$
    ).pipe(
      switchMap(() => {
        const tagIds = this.selectedTags.value;
        return this.eventService.getPublicEvents(
          tagIds?.length ? tagIds : undefined
        );
      }),
      shareReplay({ bufferSize: 1, refCount: true })
    );

    if (this.isAuthenticated) {
      this.userEvents$ = this.refreshUserEvents$.pipe(
        startWith(undefined),
        switchMap(() => this.eventService.getUserEvents()),
        shareReplay(1)
      );
    }

    this.submitResult$ = this.actionTrigger$.pipe(
      switchMap(({ type, id }) => {
        const action$ =
          type === 'join'
            ? this.eventService.joinEvent(id)
            : this.eventService.leaveEvent(id);
        return action$.pipe(
          tap(() => {
            this.refreshUserEvents$.next();
            this.refreshEvents$.next();
          }),
          map(() => ({
            success: true,
            message:
              type === 'join'
                ? 'Joined event successfully!'
                : 'Left event successfully!',
          })),
          catchError((err) =>
            of({
              success: false,
              message:
                err.error?.error ||
                (type === 'join'
                  ? 'Failed to join event'
                  : 'Failed to leave event'),
            })
          )
        );
      }),
      startWith(null)
    );
  }

  joinEvent(id: string) {
    this.actionTrigger$.next({ type: 'join', id });
  }

  leaveEvent(id: string) {
    this.actionTrigger$.next({ type: 'leave', id });
  }

  isUserParticipant(eventId: string, userEvents: CalendarEventDto[]): boolean {
    return userEvents.some((e) => e.id === eventId && !e.isCreator);
  }

  get selectedTagsCount(): number {
    return this.selectedTags.value?.length ?? 0;
  }

  toggleDropdown() {
    this.dropdownOpen = !this.dropdownOpen;
  }

  toggleTag(tagId: string) {
    const current = this.selectedTags.value || [];
    const updated = current.includes(tagId)
      ? current.filter((id) => id !== tagId)
      : [...current, tagId];
    this.selectedTags.setValue(updated);
  }

  @ViewChild('dropdownRef') dropdownRef!: ElementRef;

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    if (!this.dropdownRef.nativeElement.contains(event.target)) {
      this.dropdownOpen = false;
    }
  }
}
