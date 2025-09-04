import { Component, Inject, OnInit} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog'; 

import { AngularMaterialModule } from '../../../angular-material.module';
import { AuthService } from '../../../core/services/auth';

@Component({
  selector: 'app-ticket-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, AngularMaterialModule],
  templateUrl: './ticket-form.html',
  styleUrls: ['./ticket-form.css']
})
export class TicketForm implements OnInit {
 
  ticketForm: FormGroup;
  categories = ['Billing', 'Technical', 'General'];
  priorities = ['Low', 'Medium', 'High'];
  statuses = ['Open', 'InProgress', 'Resolved', 'Closed'];
  isEditMode = false;
  isAdmin = false;

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<TicketForm>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private authService: AuthService
  ) {
    this.ticketForm = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      category: ['', Validators.required],
      priority: ['', Validators.required],
      status: [''] 
    });
  }

 
  ngOnInit(): void {
    this.isAdmin = this.authService.isAdmin();

    if (this.data) {
      this.isEditMode = true;
      this.ticketForm.patchValue(this.data);
    }

    
    if (!this.isAdmin) {
      this.ticketForm.get('status')?.disable();
    }
  }

  onSubmit(): void {
    if (this.ticketForm.invalid) {
      return;
    }
    this.dialogRef.close(this.ticketForm.value);
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
