import { EventEmitter,Component, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})

export class RegisterComponent implements OnInit {

  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup;

  model: any = {};

  constructor( private accountService: AccountService, private toastr : ToastrService,
    private formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm(){
    this.registerForm = this.formBuilder.group({
      username: ['', Validators.required],
      gender: ['male'],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', Validators.required],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]]
    })

    this.registerForm.controls.password.valueChanges.subscribe(() => {
      this.registerForm.controls.confirmPassword.updateValueAndValidity();
    })
  }


  matchValues(matchTo: string): ValidatorFn{
    return(control: AbstractControl) => {
      return control?.value === control?.parent?.controls[matchTo].value ? null : {isMatching: true}
    }
  }

  register() {

    console.log(this.registerForm.value);

    //   this.accountService.register(this.model).subscribe(response =>{
    //   console.log(response);
    //   this.cancel();
    // }, error => {
    //   console.log(error);
    // })
  }

  cancel(){
    this.cancelRegister.emit(false);
  }
}
