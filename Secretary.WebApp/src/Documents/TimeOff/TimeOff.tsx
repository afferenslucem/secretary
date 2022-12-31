import React from 'react';
import { Button, TextField } from '@mui/material';
import { useForm } from 'react-hook-form';

import './TimeOff.scss'

export default function TimeOff() {
    const { handleSubmit, register, getValues } = useForm()

    return (
        <form onSubmit={handleSubmit(() => {
            console.debug(getValues())
        })}>
            <div className="date-input">
                <TextField
                    label="Дата"
                    type="date"
                    InputLabelProps={{
                        shrink: true,
                    }}
                    {...register('dateFrom', { valueAsDate: true, required: true })} />

                <TextField
                    label="Начало"
                    type="time"
                    InputLabelProps={{
                        shrink: true,
                    }}
                    {...register('timeFrom')} />
            </div>

            <div className="date-input">
                <TextField
                    label="Дата"
                    type="date"
                    InputLabelProps={{
                        shrink: true,
                    }}
                    {...register('dateTo', { valueAsDate: true, required: true })} />

                <TextField
                    label="Начало"
                    type="time"
                    InputLabelProps={{
                        shrink: true,
                    }}
                    {...register('timeTo')} />
            </div>

            <TextField
                label="Причина"
                multiline
                maxRows="4"
                minRows="2"
                {...register('reason')} />


            <TextField
                label="Отработка"
                multiline
                maxRows="4"
                minRows="2"
                {...register('workingOff'   )} />

            <Button type="submit" color="primary">Отправить</Button>
        </form>
    )
}
