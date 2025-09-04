import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AngularMaterialModule } from '../../angular-material.module';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [CommonModule, AngularMaterialModule],
  templateUrl: './confirm-dialog.html',
})
export class ConfirmDialog { // Class name matches file name
  constructor(
    public dialogRef: MatDialogRef<ConfirmDialog>,
    @Inject(MAT_DIALOG_DATA) public data: { title: string; message: string }
  ) { }
  onConfirm(): void { this.dialogRef.close(true); }
  onDismiss(): void { this.dialogRef.close(false); }
}
