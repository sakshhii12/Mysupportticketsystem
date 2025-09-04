import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { of, throwError } from 'rxjs';
import { Router } from '@angular/router';

import { Login } from './login';
import { AuthService } from '../../../core/services/auth';
import { AngularMaterialModule } from '../../../angular-material.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

describe('Login', () => {
  let component: Login;
  let fixture: ComponentFixture<Login>;

  // We create "spy objects" for our services.
  // These are fake objects with placeholder functions.
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let toastrServiceSpy: jasmine.SpyObj<ToastrService>;

  beforeEach(async () => {
    // Create the spy objects with the methods we are going to call.
    authServiceSpy = jasmine.createSpyObj('AuthService', ['login']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    toastrServiceSpy = jasmine.createSpyObj('ToastrService', ['success', 'error']);

    await TestBed.configureTestingModule({
      imports: [
        Login, // Since it's standalone, we import the component directly
        ReactiveFormsModule,
        AngularMaterialModule,
        BrowserAnimationsModule
      ],
      // We provide our spies as the implementation for the real services.
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: ToastrService, useValue: toastrServiceSpy }
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(Login);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have an invalid form when empty', () => {
    expect(component.loginForm.valid).toBeFalsy();
  });

  it('should call authService.login on submit with a valid form', () => {
    // ARRANGE: Make the form valid.
    component.loginForm.controls['email'].setValue('test@example.com');
    component.loginForm.controls['password'].setValue('password123');
    // Tell the spy what to return when login() is called.
    authServiceSpy.login.and.returnValue(of({ token: 'test' })); // 'of' creates a successful observable

    // ACT: Call the submit method.
    component.onSubmit();

    // ASSERT: Check that the spy was called with the correct data.
    expect(authServiceSpy.login).toHaveBeenCalledWith(component.loginForm.value);
    expect(toastrServiceSpy.success).toHaveBeenCalledWith('Login successful!');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/dashboard']);
  });

  it('should show an error toastr on failed login', () => {
    // ARRANGE
    component.loginForm.controls['email'].setValue('test@example.com');
    component.loginForm.controls['password'].setValue('wrongpassword');
    // Tell the spy to return an error observable.
    authServiceSpy.login.and.returnValue(throwError(() => new Error('Login failed')));

    // ACT
    component.onSubmit();

    // ASSERT
    expect(toastrServiceSpy.error).toHaveBeenCalled();
    expect(routerSpy.navigate).not.toHaveBeenCalled(); // Make sure it didn't navigate
  });
});
