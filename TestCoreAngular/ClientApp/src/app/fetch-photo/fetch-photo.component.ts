import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-photo',
  templateUrl: './fetch-photo.component.html'
})
export class FetchPhotoComponent {
  public photos: Photo[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<Photo[]>(baseUrl + 'photo').subscribe(result => {
      this.photos = result;
    }, error => console.error(error));
  }
}

interface Photo {
  id: string;
  img_src: string;
  earth_date: string;
}
