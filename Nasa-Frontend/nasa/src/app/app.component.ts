import { Component } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { IRover } from 'src/models/i-rover';
import { IRoverResponse } from 'src/models/i-rover-response';
import { ICameraResponse } from 'src/models/i-camera-response';
import { ICamera } from 'src/models/i-camera';
import { IPhoto } from 'src/models/i-photo';
import { IPhotoResponse } from 'src/models/i-photo-response';
import { browser } from 'protractor';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  url: string = 'http://localhost:52484/api/v1/rovers/';
  title: string = 'nasa';
  selectedRover: number;
  selectedCamera: string;
  selectedDate: string;
  currentIndex: number;

  possibleRovers: IRover[];
  possibleCameras: ICamera[];
  photos: IPhoto[];

  public constructor(private http: HttpClient) {
  }

  public async ngOnInit(){
    this.selectedDate = '2016-12-15';
    this.getRovers();
  }

  public getRovers(){
    this.http.get<IRoverResponse>(this.url).subscribe(response => {
      this.possibleRovers = response.rovers;
      this.selectedRover = this.possibleRovers[0].id;
      this.getCameras();
    });
  }

  public getCameras() {
    debugger;
    this.http.get<ICameraResponse>(this.url + `${this.selectedRover}/cameras`).subscribe(cameraResponse => {
      this.possibleCameras = cameraResponse.cameras;
      this.selectedCamera = null;
      this.search();
    });
  }

  public roverChanged() {
    this.getCameras();
  }
  
  public search() {
    var params = new HttpParams();
    params = params.set('date', this.selectedDate);

    if(this.selectedCamera){
      params = params.set('camera', this.selectedCamera);
    }

    this.http.get<IPhotoResponse>(this.url + `${this.selectedRover}/photos`, {
      params: params
    }).subscribe(photoResponse => {
      this.photos = photoResponse.photos;
      this.currentIndex = 0;
    })
  }

  public previousClicked() {
    console.log('prev');
    if(this.currentIndex === 0) {
      this.currentIndex = this.photos.length - 1;
    }
    else {
      this.currentIndex--;
    }
  }

  public nextClicked() {
    console.log('next');
    this.currentIndex++;

    if(this.currentIndex === this.photos.length){
      this.currentIndex = 0;
    }
  }

  public getMaxIndex() : number {
    return this.photos.length - 1;
  }
}
