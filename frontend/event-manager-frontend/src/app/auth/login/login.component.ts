import { Component } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../auth.service';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { BehaviorSubject, catchError, of, Subject, switchMap, tap } from 'rxjs';

interface LoginResult {
  success: boolean;
  message: string;
}

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatInputModule,
    MatButtonModule,
    MatSnackBarModule,
    RouterModule,
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent {
  loginForm: FormGroup;
  private submit$ = new Subject<void>();

  isLoading$ = new BehaviorSubject(false);

  loginResult$ = this.submit$.pipe(
    tap(() => this.isLoading$.next(true)),
    switchMap(() =>
      this.authService.login(this.loginForm.value).pipe(
        switchMap(() => {
          this.router.navigate(['/events']);
          return of({
            success: true,
            message: 'Sign in successful!',
          } as LoginResult);
        }),
        catchError((error) =>
          of({
            success: false,
            message: error.error?.error || 'Sign in failed',
          } as LoginResult)
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
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  onSubmit() {
    if (this.loginForm.invalid) return;
    this.submit$.next();
  }
}
