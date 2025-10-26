import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatNativeDateModule } from '@angular/material/core';
import { BehaviorSubject, Observable, of, Subject } from 'rxjs';
import { catchError, switchMap, tap } from 'rxjs/operators';
import {
  EventService,
  EventDto,
  EventCreateDto,
  EventUpdateDto,
} from '../event.service';

interface SubmitResult {
  success: boolean;
  message: string;
}

@Component({
  selector: 'app-form',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatNativeDateModule,
  ],
  templateUrl: './form.component.html',
  styleUrls: ['./form.component.scss'],
})
export class FormComponent implements OnInit {
  eventForm: FormGroup;
  event$!: Observable<EventDto | null>;
  isLoading$ = new BehaviorSubject<boolean>(false);
  submitResult$ = new BehaviorSubject<SubmitResult | null>(null);

  private submit$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private eventService: EventService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.eventForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(100)]],
      description: [''],
      startDate: ['', Validators.required],
      startTime: ['', Validators.required],
      location: ['', [Validators.required, Validators.maxLength(200)]],
      capacity: [null, [Validators.min(1)]],
      visibility: [true, Validators.required],
    });

    this.submitResult$ = this.submit$.pipe(
      tap(() => this.isLoading$.next(true)),
      switchMap(() => {
        if (this.eventForm.invalid) {
          this.isLoading$.next(false);
          return of({
            success: false,
            message: 'Please fill in all required fields correctly',
          } as SubmitResult);
        }

        const rawDate = this.eventForm.value.startDate;
        const [hours, minutes] = this.eventForm.value.startTime
          .split(':')
          .map(Number);
        const localDate = new Date(rawDate + 'T00:00:00');
        localDate.setHours(hours, minutes);
        const utcDateTime = localDate.toISOString();

        const dto: EventCreateDto | EventUpdateDto = {
          title: this.eventForm.value.title,
          description: this.eventForm.value.description || undefined,
          startDateTime: utcDateTime,
          location: this.eventForm.value.location,
          capacity: this.eventForm.value.capacity || undefined,
          visibility: this.eventForm.value.visibility,
        };

        return this.event$.pipe(
          switchMap((event) =>
            event
              ? this.eventService.updateEvent(event.id, dto)
              : this.eventService.createEvent(dto as EventCreateDto)
          ),
          tap(() => {
            this.router.navigate(['/me/events'], {
              state: {
                message: 'Operation successfull!',
              },
            });
          }),
          catchError((err) => {
            return of({
              success: false,
              message: err.error?.error || 'Operation failed',
            } as SubmitResult);
          }),
          tap(() => this.isLoading$.next(false))
        );
      })
    ) as BehaviorSubject<SubmitResult | null>;
  }

  ngOnInit() {
    this.event$ = this.route.paramMap.pipe(
      switchMap((params) => {
        const id = params.get('id');
        if (id) {
          return this.eventService.getEventById(id).pipe(
            tap((event) => {
              if (event) this.patchForm(event);
            })
          );
        }
        return of(null);
      })
    );
  }

  private patchForm(event: EventDto) {
    const date = new Date(event.startDateTime);
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');

    this.eventForm.patchValue({
      title: event.title,
      description: event.description,
      startDate: date.toISOString().split('T')[0],
      startTime: `${hours}:${minutes}`,
      location: event.location,
      capacity: event.capacity,
      visibility: event.visibility,
    });
  }

  onSubmit() {
    console.log('On submit:', this.eventForm.status, this.eventForm.value);
    this.submit$.next();
  }

  cancel() {
    this.router.navigate(['/me/events']);
  }
}
