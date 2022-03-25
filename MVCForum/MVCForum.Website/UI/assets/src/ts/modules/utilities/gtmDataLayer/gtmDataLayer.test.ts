import { pushgtmDataLayerEvent } from './index';

beforeEach(() => {

    window.dataLayer = [];

});

describe('GTM data layer helper', () => {

    it('Throws if the event name is not provided', () => {

        expect(() => { pushgtmDataLayerEvent('', 'mockId', {
            'mockMetaProperty': 'mock'
        })}).toThrowError();

    });

    it('Throws if the event id is not provided', () => {

        expect(() => { pushgtmDataLayerEvent('mockEvent', undefined, {
            'mockMetaProperty': 'mock'
        })}).toThrowError();

    });

    it('Creates an empty dataLayer of it doesn\'t already exist', () => {

        window.dataLayer = undefined;

        pushgtmDataLayerEvent('mockEvent', 'mockId');

        expect(window.dataLayer).toHaveLength(1);
        expect(window.dataLayer[0]).toStrictEqual({
            event: 'mockEvent',
            id: 'mockId'
        });

    });

    it('Pushes an event to the data layer', () => {

        expect(window.dataLayer).toStrictEqual([]);

        pushgtmDataLayerEvent('mockEvent', 'mockId');

        expect(window.dataLayer).toHaveLength(1);
        expect(window.dataLayer[0]).toStrictEqual({
            event: 'mockEvent',
            id: 'mockId'
        });

    });

    it('Omits the id from the event if not supplied', () => {

        expect(window.dataLayer).toStrictEqual([]);

        pushgtmDataLayerEvent('mockEvent', '');

        expect(window.dataLayer).toHaveLength(1);
        expect(window.dataLayer[0]).toStrictEqual({
            event: 'mockEvent'
        });

    });

    it('Pushes an event to the data layer with optional meta data', () => {

        expect(window.dataLayer).toStrictEqual([]);

        pushgtmDataLayerEvent('mockEvent', 'mockId', {
            'mockMetaProperty': 'mock'
        });

        expect(window.dataLayer).toHaveLength(1);
        expect(window.dataLayer[0]).toStrictEqual({
            event: 'mockEvent',
            id: 'mockId',
            mockMetaProperty: 'mock'
        });

    });

});
