import { matchText } from './index';

describe('Text matcher', () => {

    it('Should match the given term in the the string and enclose the match with <mark> </mark>', () => {

        expect(matchText({
            value: 'Just want to test the matcher by asking it to find test in this string and ignore those that are attributes of a html such as <a href="testing" data-test="test">test link</a>',
            term: 'test' 
        })).toBe('Just want to <mark>test</mark> the matcher by asking it to find <mark>test</mark> in this string and ignore those that are attributes of a html such as <a href="testing" data-test="test"><mark>test</mark> link</a>');

    });

    it('Should pass through an ineligible value unformatted', () => {

        expect(matchText({
            value: 1,
            term: 'test' 
        })).toBe(1);

    });

});
