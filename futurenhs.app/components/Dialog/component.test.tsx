import { cleanup, render, screen } from '@testing-library/react';

import { Dialog } from './index';

import { Props } from './interfaces';

describe('Dialog', () => {
    
    const props: Props = {
        id: 'Mock id',
        appElement: document.body,
        text: {
            confirmButton: 'Confirm',
            cancelButton: 'Cancel'
        },
        isOpen: true,
        cancelAction: jest.fn(),
        confirmAction: jest.fn(),
        children: <p>Child</p>
    }

    it('renders correctly', () => {
        
        render(<div id="__next"><Dialog {...props}/></div>);

        expect(screen.getAllByText('Child').length).toBe(1);

    });

    it('Conditionally renders confirm button', () => {

        render(<div id="__next"><Dialog {...props}/></div>);

        expect(screen.getAllByText('Confirm').length).toBe(1);

        cleanup();

        const propsCopy: Props = Object.assign({}, props, {
            text: null
        });

        render(<Dialog {...propsCopy}/>);

        expect(screen.queryByText('Confirm')).toBeNull();

    });

    it('conditionally renders cancel button', () => {

        render(<div id="__next"><Dialog {...props}/></div>);

        expect(screen.getAllByText('Cancel').length).toBe(1);

        cleanup();

        const propsCopy: Props = Object.assign({}, props, {
            text: null
        });

        render(<Dialog {...propsCopy}/>);

        expect(screen.queryByText('Cancel')).toBeNull();
        
    });

});