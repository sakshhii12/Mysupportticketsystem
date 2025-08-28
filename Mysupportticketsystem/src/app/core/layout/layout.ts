import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AngularMaterialModule } from '../../angular-material.module';

@Component({
  selector: 'app-layout',
  standalone: true,
  // We import RouterModule to use routerLink and AngularMaterialModule for the UI components.
  imports: [RouterModule, AngularMaterialModule],
  templateUrl: './layout.html',
  styleUrls: ['./layout.css']
})
export class Layout {
  // We will add a logout function here later.
}
