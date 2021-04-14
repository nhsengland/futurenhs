import { pushgtmDataLayerEvent } from './index';

beforeEach(() => {

    window.dataLayer = [];

});

describe('GTM data layer helper', () => {

    it('Pushes an event to the data layer', () => {

        expect(window.dataLayer).toStrictEqual([]);

        pushgtmDataLayerEvent('mockEvent', 'mockId');

        expect(window.dataLayer).toHaveLength(1);
        expect(window.dataLayer[0]).toStrictEqual({
            event: 'mockEvent',
            id: 'mockId'
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
