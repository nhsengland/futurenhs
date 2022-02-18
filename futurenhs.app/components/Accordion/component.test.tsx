import * as React from 'react';
import { render, fireEvent, screen, createEvent } from '@testing-library/react';

import { Accordion } from './index';

import { Props } from './interfaces';

const testProps: Props = {
    id: 'mock',
    isOpen: false,
    toggleChildren: <p>Toggle</p>,
    children: <p>Body</p>
};

describe('Accordion', () => {

    it('renders correctly', () => {

        const props: Props = Object.assign({}, testProps);

        render(<Accordion {...props} />);

        expect(screen.getByText('Toggle'));

    });
    
    it('toggles correctly', async () =>  {
        
        const props: Props = Object.assign({}, testProps);
        
        render(<Accordion {...props} />);
        
        fireEvent.click(screen.getByText('Toggle'));
        
        expect(await screen.findByText("Body")).toBeVisible();
        
        fireEvent.click(screen.getByText('Toggle'));
        
        expect(await screen.findByText("Body")).not.toBeVisible();
        
    });
    
    it('renders expanded when shouldMountExpanded is true', async () =>  {

        const props: Props = Object.assign({}, testProps, {
            isOpen: true
        } as Props);

        render(<Accordion {...props} />);

        expect(await screen.findByText("Body")).toBeVisible();

    });
    
    it('does not allow interaction when disabled', async () =>  {

        const props: Props = Object.assign({}, testProps, {
            isDisabled: true
        });

        render(<Accordion {...props} />);

        // to check prevent default gets called, we need to create an event to check it is prevented
        const mockEvent = createEvent.click(screen.getByText('Toggle'))

        fireEvent(screen.getByText('Toggle'), mockEvent)

        expect(mockEvent.defaultPrevented).toBeTruthy();

    });

    it('collapses on leave', async () =>  {

        const props: Props = Object.assign({}, testProps, {
            id: 'mock',
            shouldCloseOnLeave: true
        } as Props);

        render(<Accordion {...props} />);

        // open accordion
        fireEvent.click(screen.getByText('Toggle'));
        
        await screen.findByText('Body');
        expect(screen.getByText('Body'));

        // click away from accordion
        fireEvent.click(document);

        expect(await screen.findByText("Body")).not.toBeVisible();
                
    });

});
