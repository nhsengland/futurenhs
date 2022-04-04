import { email } from './index';

const testValidationMethodData = {
    type: 'email',
    message: 'test'
};

describe('Email validator', () => {

    it('Should return undefined when there is no submitted value', () => {

        const validator: Function = email(testValidationMethodData);

        expect(validator()).toBeUndefined();

    });

    it('Should return undefined when the submitted value is a valid email address', () => {

        const validator: Function = email(testValidationMethodData);

        expect(validator('test@test.com')).toBeUndefined();

    });

    it('Should return an error message when the submitted value is not a valid email address', () => {

        const validator: Function = email(testValidationMethodData);

        expect(validator('not an email!')).toBe(testValidationMethodData.message);

    });

    it('Should throw an error when invalid value is passed', () => {

        console.error = jest.fn();

        const validator: Function = email(null);

        expect(validator('test')).toBe('An unexpected error occured');

    });

});
