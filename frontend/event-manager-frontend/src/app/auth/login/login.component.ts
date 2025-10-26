import { Component } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../auth.service';
import { CommonModule } from '@angular/common';
import {
  BehaviorSubject,
  catchError,
  concat,
  map,
  Observable,
  of,
  Subject,
  switchMap,
  tap,
  timer,
} from 'rxjs';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent {
  loginForm: FormGroup;
  private submit$ = new Subject<void>();
  isLoading$ = new BehaviorSubject(false);
  temporaryMessage$: Observable<string | null>;

  loginResult$ = this.submit$.pipe(
    tap(() => this.isLoading$.next(true)),
    switchMap(() =>
      this.authService.login(this.loginForm.value).pipe(
        tap(() =>
          this.router.navigate(['/events'], {
            state: {
              message: 'Sign in successful!',
            },
          })
        ),
        map(() => null),
        catchError((error) =>
          of(error.error?.error || 'Sign in failed. Please try again.')
        ),
        tap(() => this.isLoading$.next(false))
      )
    )
  );

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
    });
    const nav = this.router.getCurrentNavigation();
    const stateMessage = nav?.extras?.state?.['message'] || null;
    this.temporaryMessage$ = stateMessage
      ? concat(of(stateMessage), timer(3000).pipe(map(() => null)))
      : of(null);
  }

  onSubmit() {
    if (this.loginForm.invalid) return;
    this.submit$.next();
  }
}
