import { validate } from './index';

describe('Validate helpers', () => {

    it('validates a correct submission', () => {

        const passResponse = validate({
            mockField: 'value'
        }, [
            {
                name: 'mockField',
                component: 'textInput',
                text: {
                    label: 'mockLabelText'
                },
                validators: [
                    {
                        type: 'required',
                        message: 'mockMessage'
                    }
                ]
            }
        ]);

        expect(passResponse).toStrictEqual({});

    });

    it('errors on an incorrect submission', () => {

        const errorResponse = validate({
            mockField: ''
        }, [
            {
                name: 'mockField',
                component: 'textInput',
                text: {
                    label: 'mockLabelText'
                },
                validators: [
                    {
                        type: 'required',
                        message: 'mockMessage'
                    }
                ]
            }
        ]);

        expect(errorResponse).toStrictEqual({
            mockField: 'mockMessage'
        });

    });
    
});
