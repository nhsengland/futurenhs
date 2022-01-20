export const logInFormFieldConfig = [
    {
        component: 'input',
        name: 'userName',
        text: {
            label: 'Email address'
        },
        validators: [
            {
                type: 'required',
                message: 'Enter your registered email address'
            }
        ]
    },
    {
        component: 'input',
        name: 'passWord',
        inputType: 'password',
        text: {
            label: 'Password'
        },
        validators: [
            {
                type: 'required',
                message: 'Enter your password'
            }
        ]
    }
]; 