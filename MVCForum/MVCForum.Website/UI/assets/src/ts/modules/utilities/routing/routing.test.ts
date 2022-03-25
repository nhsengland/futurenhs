import { getObjectFromSearchParams, getSearchParamsFromObject, ping } from './index';

describe('Routing helpers: getObjectFromSearchParams', () => {

    it('Returns any empty object for when given empty parameters', () => {

        expect(getObjectFromSearchParams('')).toStrictEqual({});

    });

    it('Returns the correct object for given search parameters', () => {

        expect(getObjectFromSearchParams('?foo=bar&baz=foo')).toStrictEqual({
            foo: 'bar',
            baz: 'foo'
        });

    });

});

describe('Routing helpers: getSearchParamsFromObject', () => {

    it('Returns any empty string when given a falsey object', () => {

        expect(getSearchParamsFromObject()).toEqual('');
        expect(getSearchParamsFromObject({})).toEqual('');

    });

    it('Returns the correct search string for given object', () => {

        expect(getSearchParamsFromObject({
            foo: 'bar',
            baz: undefined
        })).toBe('?foo=bar&baz=');

    });

});

describe('Routing helpers: ping', () => {

    it('Calls getJson with the given endpoint', () => {

        const spy = jest.spyOn((jQuery as any), 'getJSON');

        ping('/mock');

        expect(spy).toHaveBeenCalledTimes(1);

    });

});
