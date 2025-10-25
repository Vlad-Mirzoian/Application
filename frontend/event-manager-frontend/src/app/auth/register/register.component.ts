import { Component } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../auth.service';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { BehaviorSubject, catchError, of, Subject, switchMap, tap } from 'rxjs';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatInputModule,
    MatButtonModule,
    RouterModule,
  ],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent {
  registerForm: FormGroup;
  private submit$ = new Subject<void>();
  isLoading$ = new BehaviorSubject(false);

  registerResult$ = this.submit$.pipe(
    tap(() => this.isLoading$.next(true)),
    switchMap(() =>
      this.authService.register(this.registerForm.value).pipe(
        tap(() => {
          this.router.navigate(['/auth/login'], {
            state: {
              message: 'Sign up successful! You can now sign in.',
            },
          });
        }),
        catchError((error) =>
          of(error.error?.error || 'Sign up failed. Please try again.')
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
    this.registerForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
    });
  }

  async onSubmit() {
    if (this.registerForm.invalid) return;
    this.submit$.next();
  }
}
