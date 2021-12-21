import { capitalise } from './index';

describe('Capitalise formatter', () => {

    it('Should convert the first character of a string to uppercase', () => {

        const formatter: Function = capitalise();

        expect(formatter('mock')).toBe('Mock');

    });

});
