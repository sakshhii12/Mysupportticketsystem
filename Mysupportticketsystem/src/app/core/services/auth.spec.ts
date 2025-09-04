import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { Router } from '@angular/router';

import { AuthService } from './auth';

// A "describe" block groups related tests together.
describe('AuthService', () => {
  let service: AuthService;
  let httpTestingController: HttpTestingController;
  let router: Router;

  // The "beforeEach" block runs before each test ("it" block).
  beforeEach(() => {
    TestBed.configureTestingModule({
      // We import special testing modules that provide fake versions of services.
      imports: [
        HttpClientTestingModule, // Provides a mock HttpClient
        RouterTestingModule      // Provides a mock Router
      ],
      providers: [AuthService]
    });

    // We use the TestBed to get instances of our service and its mocked dependencies.
    service = TestBed.inject(AuthService);
    httpTestingController = TestBed.inject(HttpTestingController);
    router = TestBed.inject(Router);
  });

  // A simple test to make sure the service can be created.
  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  // --- A real test for the login() method ---
  it('should set tokens in localStorage on successful login', () => {
    // ARRANGE: Set up our test conditions.
    const mockCredentials = { email: 'test@example.com', password: 'password' };
    const mockResponse = { token: 'jwt-token', refreshToken: 'refresh-token' };

    // Use a Jasmine spy to watch localStorage.setItem.
    const setItemSpy = spyOn(localStorage, 'setItem').and.callThrough();

    // ACT: Call the method we are testing.
    service.login(mockCredentials).subscribe();

    // ASSERT: Check the HTTP request.
    const req = httpTestingController.expectOne('/api/auth/login');
    expect(req.request.method).toEqual('POST');
    expect(req.request.body).toEqual(mockCredentials);

    // "Flush" the request with our mock response to complete the subscribe() call.
    req.flush(mockResponse);

    // Now check the side effect.
    expect(setItemSpy).toHaveBeenCalledWith('access_token', 'jwt-token');
    expect(setItemSpy).toHaveBeenCalledWith('refresh_token', 'refresh-token');
  });

  // --- A real test for the logout() method ---
  it('should clear tokens and navigate to login on logout', () => {
    // ARRANGE
    spyOn(localStorage, 'clear').and.callThrough();
    const navigateSpy = spyOn(router, 'navigate');

    // ACT
    service.logout();

    // ASSERT
    expect(localStorage.clear).toHaveBeenCalled();
    expect(navigateSpy).toHaveBeenCalledWith(['/login']);
  });

});
