import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

import { AngularMaterialModule } from '../../../angular-material.module';
import { AuthService } from '../../../core/services/auth'; // <-- Adjusted import path

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    AngularMaterialModule
  ],
  templateUrl: './login.html', // Corresponds to your login.html file
  styleUrls: ['./login.css']    // Corresponds to your login.css file
})
export class Login {
  loginForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private toastr: ToastrService
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.toastr.error('Please enter valid credentials.');
      return;
    }

    this.authService.login(this.loginForm.value).subscribe({
      next: () => {
        this.toastr.success('Login successful!');
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        // Check if there is a specific error message from the backend
        const errorMessage = err.error?.message || 'Login failed. Please check your email and password.';
        this.toastr.error(errorMessage, 'Error');
      }
    });
  }
}
