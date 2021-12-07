import { required } from './index';

const testValidationMethodData = {
  type: 'required',
  message: 'test'
};

describe('Required validator', () => {

  it('Should return an error message when the submitted value is falsey', () => {

    const validator: Function = required(testValidationMethodData);

    expect(validator()).toBe(testValidationMethodData['message']);

  });

  it('Should return an error message when the submitted value is an array which is empty', () => {

    const validator: Function = required(testValidationMethodData);

    expect(validator([])).toBe(testValidationMethodData['message']);

  });

  it('Should return an error message when the submitted value is a string which is empty', () => {

    const validator: Function = required(testValidationMethodData);

    expect(validator('    ')).toBe(testValidationMethodData['message']);

  });

  it('Should return undefined when the submitted value is truthy', () => {

    const validator: Function = required(testValidationMethodData);

    expect(validator('test')).toBeUndefined();

  });

  it('Should return undefined when the submitted value is an array with values', () => {

    const validator: Function = required(testValidationMethodData);

    expect(validator(['test'])).toBeUndefined();

  });

});
