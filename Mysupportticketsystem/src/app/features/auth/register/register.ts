import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

import { AngularMaterialModule } from '../../../angular-material.module';
import { AuthService } from '../../../core/services/auth';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    AngularMaterialModule
  ],
  templateUrl: './register.html',
  styleUrls: ['./register.css']
})
export class Register {
  registerForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private toastr: ToastrService
  ) {
    this.registerForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.toastr.error('Please enter a valid email and a password of at least 6 characters.');
      return;
    }

    this.authService.register(this.registerForm.value).subscribe({
     
      next: () => {
        this.toastr.success('Registration successful! Please log in.', 'Success');
        this.router.navigate(['/login']);
      },

     
      error: (err) => {
        let errorMessage = 'Registration failed. Please try again.';

       
        if (err.error && typeof err.error === 'string') {
          errorMessage = err.error;
        }
        
        else if (err.error && typeof err.error === 'object' && err.error.errors) {
          const validationErrors = err.error.errors;
          const errorMessages = Object.values(validationErrors).flat();
          if (errorMessages.length > 0) {
            errorMessage = errorMessages.join(' ');
          }
        }
        
        this.toastr.error(errorMessage, 'Error');
      }
    });
  }
}
