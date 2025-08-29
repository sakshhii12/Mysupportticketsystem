import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  title = 'Mysupportticketsystem';
}

//add loader
// change controller structure
//project struct

//first presentation layer - client app (web api project)
//application layer - controller, appsettinings(class library)
//DAL - migration, db, whatever in db(class library)
//core - handlers, common services(class library),policies
//
